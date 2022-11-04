import $ from 'jquery'

//TODO script SETTINGS
var guestUniqueId = "1";
var guestGuid = "";
var companyId = "1";
var companyHash = "AB9D4D56C7374078E35F8AFFA1F7D80B829429CB";


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
            'GuestUniqueId': guestUniqueId,
            'GuestGuid': guestGuid,
            'CompanyHash': companyHash,
            'CompanyId': companyId,
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