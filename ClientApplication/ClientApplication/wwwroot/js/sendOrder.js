function sendOrder(id) {
    var position = [];
    var quantity = [];

    var inputs = document.getElementsByTagName('input');

    for (var i = 0; i < inputs.length; i++) {
        if (inputs[i].type.toLowerCase() == 'number') {
            if (inputs[i].value > 0) {
                position.push(inputs[i].id);
                quantity.push(inputs[i].value);
            }
        }
    }

    if (position.length == 0 || quantity.length == 0) {
        alert("Please pick dish you would like to order!");
        return;
    }

    var city = document.getElementById('city').value;
    var street = document.getElementById('street').value;
    var postCode = document.getElementById('postCode').value;
    var addressDTO = null;

    if (city || street || postCode) {
        if (city && street && postCode) {
            var reg = /^\d{2}[-]\d{3}$/;
            if (!reg.test(postCode)) {
                alert("Post code must be in format XX-XXX!");
                return;
            }
            addressDTO = { "city": city, "street": street, "postCode": postCode };
        } else {
            alert("Please enter all address data!");
            return;
        }
    }


    var select = document.getElementById('paymentMethod');
    var payment = select.options[select.selectedIndex].text;

    if (!payment.localeCompare("Please select payment method")) {
        alert("Please pick payment method!");
        return;
    }

    var flag = true;
    var discountId = document.getElementById('discountID').value;
    if (!discountId) {
        discountId = null;
    } else {
        $.ajax({
            async: false,
            type: 'GET',
            url: '/Restaurants/CheckDiscountCode?discountCode=' + discountId + '&restaurantId=' + id,
            success: function (data) {
                if (data == false) {
                    alert("Discount Code is invalid!");
                    flag = false;
                }
            }
        });
    }

    if (!flag)
        return;

    $.ajax({
        type: 'POST',
        url: '/Restaurants/Restaurant',
        data: {
            "paymentMethod": payment,
            "discountcodeId": discountId,
            "restaurantId": id,
            "positions": position,
            "quantities": quantity,
            "address": addressDTO
        },
        success: function (response) {
                alert("Order sent succesfully!")
        },
        error: function (ts) {
            alert(ts.responseText)
        }
    });

}