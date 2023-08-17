$(document).ready(() => {
    function showTable() {
        const url = $(this).data('url');
        const userId = $(this).data('user');
        let data = {};
        if (userId) data.userId = userId;
        /*console.log(url);*/
        $.ajax({
            url: url,
            data: data,
            method: 'POST',
            dataType: "json",
            success: function (result) {
                /*console.log(result);*/
                $('#result').html(`
                   <table id="myTable" class="table table-stripped">
                        <thead>
                            <tr>
                                <th>Айді</th>
                                <th>Повідомлення</th>
                                <th>Дата</th>
                            </tr>
                        </thead>
                        <tbody>
                            ${result.map(row => `
                                <tr>
                                    <td>${row.userId}</td>
                                    <td>${row.text}</td>
                                    <td>${row.timeOfCreation}</td>
                                </tr>
                            `).join('')}
                        </tbody>
                    </table>
                `);

            },
            error: function (error) {
                console.error('error');
            }
        });
    }

    $('#show10UseMessages').click(showTable);
    $('#show20UsersMessages').click(showTable);

    
});
