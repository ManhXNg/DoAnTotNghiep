
$(document).ready(function () {

    window.onscroll = function () {
        var navbar = document.getElementById("navbar-header");
        if (window.pageYOffset > 50) {
            navbar.classList.add("scroll");
        } else {
            navbar.classList.remove("scroll");
        }
    };
    var prevScrollpos = window.pageYOffset;
    window.onscroll = function () {
        var currentScrollPos = window.pageYOffset;
        if (prevScrollpos > currentScrollPos) {
            // Kéo lên trang
            document.getElementById("navbar-header").classList.add("navbar-visible");
            document.getElementById("navbar-header").classList.remove("navbar-hidden");
        } else {
            // Kéo xuống trang
            document.getElementById("navbar-header").classList.add("navbar-hidden");
            document.getElementById("navbar-header").classList.remove("navbar-visible");
        }
        prevScrollpos = currentScrollPos;
    };
    $("#btn-register").click(function () {
        let name = $('#r_nameCustomer').val();
        let phone = $('#r_phoneNumber').val();
        let email = $('#r_emailCustomer').val();
        let password1 = $('#r_Password1').val();
        let password2 = $('#r_Password2').val();
        let sex = $('input[name="gender"]:checked').val();
        let dateOfBirth = $('#r_datebirth').val();
        let address = $('#r_address').val();

        // Validate name field
        if (name == '') {
            $('#nameHelp').html('Vui lòng nhập tên của bạn.');
            $('#nameHelp').removeClass('d-none');
            return false;
        } else {
            $('#nameHelp').addClass('d-none');
        }

        // Validate phone field
        let phonePattern = /(03|05|07|08|09)+([0-9]{8})\b/g;
        if (!phonePattern.test(phone)) {
            $('#phoneHelp').html('Số điện thoại không hợp lệ!');
            $('#phoneHelp').removeClass('d-none');
            return false;
        } else {
            $('#phoneHelp').addClass('d-none');
        }

        // Validate email field
        if (email == '') {
            $('#emailHelp').html('Vui lòng nhập địa chỉ email của bạn.');
            $('#emailHelp').removeClass('d-none');
            return false;
        } else if (!isValidEmail(email)) {
            $('#emailHelp').html('Vui lòng nhập địa chỉ email hợp lệ.');
            $('#emailHelp').removeClass('d-none');
            return false;
        } else {
            $('#emailHelp').addClass('d-none');
        }

        // Validate password fields
        if (password1 == '') {
            $('#passwordHelp').html('Vui lòng nhập mật khẩu.');
            $('#passwordHelp').removeClass('d-none');
            return false;
        } else if (password1 != password2) {
            $('#passwordHelp').html('Mật khẩu nhập lại không khớp.');
            $('#passwordHelp').removeClass('d-none');
            return false;
        } else {
            $('#passwordHelp').addClass('d-none');
        }

        // Validate address field
        if (address == "") {
            $("#addressHelp").html("Vui lòng nhập địa chỉ của bạn");
            $("#addresssHelp").removeClass("d-none");
            return false;
        } else {
            $("#addressHelp").addClass("d-none");
        }

        // Validate date of birth field
        if (dateOfBirth == "") {
            $("#datebirthHelp").html("Vui lòng chọn ngày sinh của bạn");
            $("#datebirthHelp").removeClass("d-none");
            return false;
        } else {
            $("#datebirthHelp").addClass("d-none");
        }

        // Validate gender field
        if (sex === undefined) {
            $("#sexHelp").html("Vui lòng chọn giới tính của bạn");
            $("#sexHelp").removeClass("d-none");
            return false;
        }
        else {
            $("#sexHelp").addClass("d-none");
        }
       
        
        let obj =  {
            FullName: name,
            Phone: phone,
            Email: email,
            Password: password1,
            DateBirth: dateOfBirth,
            Sex: sex == 'true' ? true : false,
            Address: address,
        };
        // Send data to server using AJAX in JSON format
        $.ajax({
            type: 'POST',
            url: '/Login/Register',
            contentType: 'application/json',
            data: JSON.stringify(obj) ,
            success: function (response) {
                if (response.success == true) {
                    MessageSucces(response.mess);
                    setTimeout(function () {
                        location.reload();
                    }, 1500); // 5000 milliseconds = 5 seconds

                } else {
                    MessageError(response.mess)
                }

            },
            error: function (xhr, status, error) {
                // Handle error response from server
                // ...
            }
        });
    });
    $("#btn-login").click(function () {

        var email = $('#login_email').val();
        var password = $('#login_password').val();


        // Validate name field
        if (email == '') {
            $('#l_emailHelp').html('Vui lòng nhập email');
            return false;
        } if (password == '') {
            $('#l_emailHelp').html('Vui lòng nhập mật khẩu');
            return false;
        }



        // Send data to server using AJAX in JSON format
        $.ajax({
            type: 'POST',
            url: '/Login/Login',
            contentType: 'application/json',
            data: JSON.stringify({
                Email: email,
                Password: password
            }),
            success: function (response) {
                if (response.success == true) {
                    MessageSucces(response.mess);
                    setTimeout(function () {
                        location.reload();
                    }, 1500); // 5000 milliseconds = 5 seconds

                } else {
                    MessageError(response.mess, 5000)
                }

            },
            error: function (xhr, status, error) {
                // Handle error response from server
                // ...
            }
        });
    });
    $("#btn-change-password").click(function () {

        var password_old = $('#password-old').val();
        var password_new = $('#password-new').val();
        var password_new2 = $('#password-new2').val();


        // Validate name field
        if (password_old == '') {
            $('#password-old-mess').html('Vui lòng nhập mật khẩu cũ');
            return false;
        } if (password_new == '') {
            $('#password-new-mess').html('Vui lòng nhập mật mới');
            return false;
        }
        if (password_new2 == '') {
            $('#password-new2-mess').html('Vui lòng nhập mật mới');
            return false;
        }
        if (password_new != password_new2) {
            $('#password-new2-mess').html('Nhập lại mật khẩu mới không giống nhau');
            return false;
        }



        // Send data to server using AJAX in JSON format
        $.ajax({
            type: 'POST',
            url: '/Customer/ChangePassword',

            data: {
                passwordnew: password_new,
                passwordold: password_old,
            },
            success: function (response) {
                if (response == 1) {
                    MessageSucces("Đổi mật khẩu thành công");
                    setTimeout(function () {
                        location.reload();
                    }, 1500); // 5000 milliseconds = 5 seconds
                } else if (response == 2) {
                    MessageError("Vui lòng nhập mật khẩu mới, mật khẩu bạn vừa nhập là mật khẩu cũ");
                }
                else if (response == 3) {
                    MessageError("Mật khẩu cũ không đúng");
                }
                else {
                    MessageError("Đổi mật khẩu thất bại", 5000);
                }

            },
            error: function (xhr, status, error) {
                // Handle error response from server
                // ...
            }
        });
    });

    function isValidEmail(email) {
        var pattern = /^\w+@[a-zA-Z_]+?\.[a-zA-Z]{2,3}$/;
        return pattern.test(email);
    }

    
});
