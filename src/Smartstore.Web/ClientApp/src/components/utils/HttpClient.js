import $ from 'jquery'
var companyHash = "asd";
var customerId = "1";
var encodeHash = "2";

const LauncherUrl = 'https://localhost:44442';

export const get = async (url) => {
    const response = await fetch(url);

    const jsonData = await response.json();

    return jsonData;
};

export const getNotAsync = (url) => {
    debugger;
    const response = fetch(url);

    const jsonData = response.json();

    return jsonData;
};


export const post = async (url, object) => {
    var token = $('meta[name="__rvt"]').attr("content") || $('input[name="__RequestVerificationToken"]').val();

    const response = await fetch(url, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'X-XSRF-Token': encodeURIComponent(token)
        },
        credentials: 'include',
        body: JSON.stringify(object)
    });

    const jsonData = await response.json();

    return jsonData;
};

export const postLauncher = async (url, object) => {
    //url = `${url}`;
    var token = $('meta[name="__rvt"]').attr("content") || $('input[name="__RequestVerificationToken"]').val();

    const response = await fetch(url, {
        method: 'POST',
        //mode: "no-cors",
        headers: {
            "Content-Type": "application/json",
            'X-XSRF-Token': encodeURIComponent(token),
            'CompanyCustomerId': customerId,
            'CompanyHash': companyHash,
            'EncodeHash': encodeHash
        },
        credentials: 'include',
        body: JSON.stringify(object)
    });

    const jsonData = await response.json();

    return jsonData;
};

export const postData = async (url, object) => {
    var token = $('meta[name="__rvt"]').attr("content") || $('input[name="__RequestVerificationToken"]').val();

    return new Promise(function (resolve, reject) {
        $.ajax({
            url: url,
            type: "POST",
            headers: {
                'X-XSRF-Token': encodeURIComponent(token)
            },
            data: object,
            beforeSend: function () {
            },
            success: function (response) {
                return resolve($.parseJSON(response)) // Resolve promise and when success
            },
            error: function (err) {
                reject(err) // Reject the promise and go to catch()
            }
        });
    });
};