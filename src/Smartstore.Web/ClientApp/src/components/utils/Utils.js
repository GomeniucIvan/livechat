import $ from 'jQuery';
import { useEffect, useRef } from "react";
import React from 'react'
import lang from './Translate.Languages';
import RSVG from './Rsvg';

let defTitle = '';
let routeTitle = '';

export const enableButtons = () => {
    $(".btn-spinner .spinner").remove();
    $('.btn-spinner').removeClass("disabled");
    $('.btn-spinner').removeClass("btn-spinner");
};

export const isNullOrEmpty = (data) => {
    if (data === undefined || data === '' || data === null || data === 'undefined') {
        return true;
    }

    var stringData = data.toString();

    try {
        stringData = stringData.trim();
    } catch (e) {
        return true;
    }

    return (!stringData || 0 === stringData.length);
}

export function getCurrentUrl(pathname: string) {
    return pathname.split(/[?#]/)[0]
}

export const showResponsePNotifyMessage = (data) => {
    if (!data) {
        return false;
    }

    if (isNullOrEmpty(data.Message)) {
        return false;
    }

    var type = data.IsValid ? "success" : "error";
    window.showPNotifyMessage(type, data.Message);
}

export const pNotifyError = (msg) => {
    window.showPNotifyMessage('error', msg);
}

export const tglDarkMode = (isDarkSelected, withoutTimeout) => {
    if (isDarkSelected) {
        window.enableDarkMode(withoutTimeout);
    } else {
        window.enableDayMode();
    }
}

export const generateRandomInteger = (min, max) => { 
    return Math.floor(Math.random() * (max - min + 1) + min)
}

export const useComponentDidMount = handler => {
    return useEffect(() => handler(), []);
};

export const useComponentDidUpdate = (handler, deps) => {
    const isInitialMount = useRef(true);

    useEffect(() => {
        if (isInitialMount.current) {
            isInitialMount.current = false;

            return;
        }

        return handler();
    }, deps);
};

export const useComponentWillUnmount = handler => {
    return useEffect(() => handler, []);
};

export const setHeaderData = (data) => {
    defTitle = data.DefaultTitle;
    routeTitle = data.RouteTitle;
}

export const stringFormat = (text, args) => {
    if (isNullOrEmpty(args) || args.length === 0) {
        return text;
    }

    var s = text,
        i = args.length;

    while (i--) {
        s = s.replace(new RegExp('\\{' + i + '\\}', 'gm'), args[i]);
    }
    return s;
}

export const formatTitle = (incTitle) => {
    if (isNullOrEmpty(incTitle)) {
        return defTitle;
    }

    let args = [];
    args.push(incTitle);

    return stringFormat(routeTitle, args);
}
export const tglItem = (isChecked, ctx, animate) => {
    var show = isChecked,
        reverse = false;

    if (reverse) show = !show;

    var duration = animate ? 200 : 0;

    function afterShow() { $(this).addClass('expanded'); }
    function afterHide() { $(this).removeClass('expanded'); }

    const cel = $(ctx.current);
    var pnl = $(cel),
        isGroup = pnl.is('tbody, .collapsible-group'),
        reversePanel = pnl.data('panel-reverse'),
        showPanel = show;

    if (reversePanel) showPanel = !showPanel;

    pnl.addClass('collapsible');
    if (isGroup) pnl.addClass('collapsible-group');

    if (showPanel) {
        if (!isGroup) {
            pnl.show(duration, afterShow);
        }
        else {
            var targets = pnl.children()
                .hide() // initially hide all children asap
                .filter(':not(.collapsible), .collapsible.expanded'); // fetch only expandable items
            pnl.show(0, afterShow); // first, show panel group asap (otherwise we won't see any animation)
            targets.show(duration); // animate all items
        }
    }
    else {
        if (!isGroup) {
            pnl.hide(duration, afterHide);
        }
        else {
            // hide all children (animated)
            pnl.children().hide(duration).promise().done(function () {
                pnl.hide(0, afterHide); // last, hide panel group asap
            });
        }
    }
}

export const truncate = (str, length) => {
    return str.length > length ? str.substring(0, length - 3) + '...' : str;
}
export const equal = (firstItem, secondItem) => {
    if (isNullOrEmpty(firstItem) && isNullOrEmpty(secondItem)) {
        return true;
    }

    if (!isNullOrEmpty(firstItem) && isNullOrEmpty(secondItem)) {
        return false;
    }

    if (isNullOrEmpty(firstItem) && !isNullOrEmpty(secondItem)) {
        return false;
    }

    const firstItemToString = firstItem.toString().toLowerCase();
    const secondItemToString = secondItem.toString().toLowerCase();

    return firstItemToString === secondItemToString;
}

export const KTSVG = ({ className = '', icon, svgClassName = 'mh-50px', width = 20, height = 20 }) => {
    return (       
        <span className={`svg-icon ${className}`}>
            <svg width={width} height={height} fill="none" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg" className={svgClassName}>
                <path d={RSVG[icon]} fill="currentColor" />
            </svg>
        </span>
    )
}

export const toAbsoluteUrl = (pathname: string) => process.env.PUBLIC_URL + pathname;

export const T = (text, arg1, args2) => {
    var args = [];

    if (!isNullOrEmpty(arg1)) {
        args.push(arg1);
    }

    if (!isNullOrEmpty(args2)) {
        args.push(args2);
    }

    let resource = lang.resources.find((o) => { return o["ResourceName"] === text });
    if (isNullOrEmpty(resource)) {
        resource = text;
    }
    return stringFormat(resource, args);
};

function afterShow() { $(this).addClass('expanded'); }
function afterHide() { $(this).removeClass('expanded'); }
export function showHide(target, hide) {
    var ctl = $(target);
    var duration = 200;

    if (!hide) {
        ctl.show(duration, afterShow);
    } else {
        ctl.hide(duration, afterHide);
    }

    return false;
}

export function showAndHide(targetToShow, targetToHide, duration) {
    var ctlToShow = $(targetToShow);
    var ctlToHide = $(targetToHide);
    var duration = duration ?? 200;

    ctlToHide.hide(duration, afterHide);

    setTimeout(function () { ctlToShow.show(duration, afterShow); }, duration);
    return false;
}