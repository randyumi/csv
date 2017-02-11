<?php

namespace App\Http\Controllers;

use Illuminate\Http\Request;
use DB;

class TestController extends Controller
{
    public function index() {
        return view('topics.input', compact('topics'));
    }

    public function putCsv(Request $request) {
        $file_name = $request->file('csv');
        if($file_name) {
            $csv = $request->file('csv');
            $f = fopen($csv, 'r');
            mb_internal_encoding("UTF-8");
            mb_regex_encoding("UTF-8");
            DB::transaction(function() use($f){
                while($line = fgets($f)) {
                    $line = trim($line);
                    $values = mb_split(",", $line);
                        if (sizeof($values) > 2 && is_numeric($values[0]) && is_numeric($values[1]) && is_numeric($values[2])) {
                            DB::statement('replace into account values(:id, :turnover, :cost)', array('id' => intval($values[0]), 'turnover' => intval($values[1]), 'cost' => intval($values[2])));
                        } else {
                        }
                }
            });
            fclose($f);
        }
        return response()->json(['status' => 'ok'], 200);
    }

    public function getRecord($id) {
        if (!is_numeric($id)) {
            return response()->json(array('errorCode' => 'invalid_parameter'))->setStatusCode(400);
        } else {
            $account = DB::table('account')->where('id', $id)->get();
            if (sizeof($account) == 1) {
                return response()->json($account[0], 200);
            } else if (sizeof($account) > 1) {
                logger("$id is duplicated on account table: $account");
                return response()->json($account[0], 200);
            } else {
                return response()->json(array('errorCode' => 'not_found'))->setStatusCode(404);
            }
        }
    }
}
