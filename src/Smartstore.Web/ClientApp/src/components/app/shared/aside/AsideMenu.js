import React, { useRef, useEffect } from 'react'
import { useLocation } from 'react-router'
import { AsideMenuMain } from './AsideMenuMain'

const AsideMenu = () => {
    const scrollRef = useRef(null);
    const { pathname } = useLocation();

    useEffect(() => {
        setTimeout(() => {
            if (scrollRef.current) {
                scrollRef.current.scrollTop = 0
            }
        }, 50)
    }, [pathname])

    return (
        <div ref={scrollRef} className='hover-scroll-overlay-y my-2 my-lg-2' >
            <div className='menu menu-column px-3' >
                <AsideMenuMain />
            </div>
        </div>
    )
}

export { AsideMenu }
