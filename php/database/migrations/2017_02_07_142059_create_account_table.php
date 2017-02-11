<?php

use Illuminate\Database\Schema\Blueprint;
use Illuminate\Database\Migrations\Migration;

class CreateAccountTable extends Migration
{
    /**
     * Run the migrations.
     *
     * @return void
     */
    public function up()
    {
        Schema::create('account', function(Blueprint $table) {
            $table->integer('id', /*auto_increment = */false, /*unsigned = */true)->primary();
            $table->integer('turnover');
            $table->integer('cost');
        });
    }

    /**
     * Reverse the migrations.
     *
     * @return void
     */
    public function down()
    {
        Schema::drop('account');
    }
}
