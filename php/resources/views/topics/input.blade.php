<!DOCTYPE HTML>
<html lang="en">
<head>
    <script type="text/javascript" src="https://code.jquery.com/jquery-3.1.1.min.js"></script>
    <title>mofu</title>
</head>
<body>
<script type="text/javascript">
$(document).ready(function() {
    $('#select').submit(function() {
        console.log($('#id').val());
        $.ajax({
            url: "{{action('TestController@getRecord', '')}}/" + $('#id').val(),
            type: 'GET',
            success: function(d) {
                $('#r_id').text(d['id']);
                $('#r_turnover').text(d['turnover']);
                $('#r_cost').text(d['cost']);
            }
        });
        return false;
    });
});
</script>
<p>Choose a csv file to upload. The contents will registered on our database.</p>
{{Form::open(['enctype'=>'multipart/form-data', 'action' => 'TestController@putCsv', 'method' => 'PUT', 'id' => 'putForm'])}}
{{Form::file('csv', ['id' => 'csv'])}}
{{Form::submit()}}
{{Form::close()}}

<p>Query a company ID.</p>
{{Form::open(['method' => 'GET', 'id' => 'select'])}}
{{Form::text('id', null, ['id' => 'id'])}}
{{Form::submit()}}
{{Form::close()}}
<p><span>id: </span><span id="r_id"></span></p>
<p><span>turnover: </span><span id="r_turnover"></span></p>
<p><span>cost: </span><span id="r_cost"></span></p>
</body>
</html>
