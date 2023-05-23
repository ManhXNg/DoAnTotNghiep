$(document).ready(function () {
    
    $.ajax({
        url: "/Home/GetDataFooter",
        type: "GET",
        success: function (result) {
            let html = ` <a class="text-dark mb-2" href="/"><i class="fa fa-angle-right mr-2"></i>Home</a>
                            <a class="text-dark mb-2" href="/Shop"><i class="fa fa-angle-right mr-2"></i>Shop</a>`;
            $.each(result, function (index,value) {
                html += `<a class="text-dark mb-2" href="/Shop?categoryid=${value.CategoryId}"><i class="fa fa-angle-right mr-2"></i>${value.CategoryName}</a>`;
            });
           

            $("#layout_footer").html(html);

        }

    });


});