

function GetAuth() {
    $.ajax({
        type: "get",
        url: "/googleads/GoogleAuth",
        success: function (res) {
      
            try {

                window.open(
                    res,
                    '_blank' // <- This is what makes it open in a new window.
                );
      
            } catch {
                console.log("error-->" + res);
            }
       
        },
      
    });
}