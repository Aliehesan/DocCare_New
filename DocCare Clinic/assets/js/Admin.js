
$(document).ready(function () {

    // jQuery methods go here...
    $('#sup-login-form').submit('click', function () {
        // $('#sup-errormsg').text('AhesanAli')

        var reg = new RegExp('^[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,4}$');
        var regpassword = new RegExp("\d{8}");

        let email = $('#sup-email').val()
        let password = $('#sup-password').val()
        const data = $('#sup-login-form').serialize()
        if (email.length != 0 && password.length != 0) {
            {
                if (reg.test(email)) {
                    if (password.length == 8) {
                        console.log(data);
                        $.ajax({
                            type: 'POST',
                            url: 'Admin_opration.php?ch=1',
                            data: data,
                            success: (resp) => {
                                console.log(resp);
                                if (resp == 1) {
                                    console.log("Response from PHP: ", resp);
                                    window.location.href = "../SuperAdmin/index.php";
                                }
                                else {
                                    $('#sup-errormsg').html('<p class="error">⚠ Invalid Credential</p>');
                                }
                            }
                        });

                    }
                    else {
                        $('#sup-errormsg').html('<p class="error">⚠ Password Must Be Eight Digit </p>')
                    }
                }
                else {
                    $('#sup-errormsg').html('<p class="error">⚠ Wrong Email Format</p>')
                }
            }
        }
        else {
            $('#sup-errormsg ').html('<p class="error"> ⚠  All Field Are Required</p>')
        }
        setTimeout(() => {
            $('#sup-errormsg p').hide()
        }, 3000);
    })
});
