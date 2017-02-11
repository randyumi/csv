<!DOCTYPE HTML>
<html lang="en">
<head>
    <title>mofu</title>
</head>
<body>
<p>Choose a csv file to upload. The contents will registered on our database.</p>
{{Form::open(['enctype'=>'multipart/form-data', 'action' => 'TestController@putCsv', 'method' => 'PUT'])}}
{{Form::file('csv')}}
{{Form::submit()}}
{{Form::close()}}

<p>Query a company ID.</p>
{{action('TestController@getRecord', '')}}
{{Form::open(['method' => 'GET'])}}
{{Form::text('id')}}
{{Form::submit()}}
{{Form::close()}}
</body>
</html>
