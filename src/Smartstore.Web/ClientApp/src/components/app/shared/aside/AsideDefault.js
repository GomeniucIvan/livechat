import React, { useEffect } from 'react'
import { Link } from 'react-router-dom'
import { AsideMenu } from './AsideMenu'
import { AsideMenuItem } from './AsideMenuItem'
import $ from 'jQuery';
import { T } from '../../../utils/Utils'

const AsideDefault = () => {
    let aside = {
        theme: 'dark',
    }

    useEffect(() => {
        let heightToSet = $(window).height() - $('.aside-logo').outerHeight() - $('.aside-footer').outerHeight();
        const bodyHeight = $('.app-sidebar-menu').outerHeight();
        if (heightToSet < bodyHeight) {
            heightToSet = bodyHeight;
        }
        $('.app-sidebar-menu').height(heightToSet);

        window.addEventListener("resize", () => {
            heightToSet = $(window).height() - $('.aside-logo').outerHeight() - $('.aside-footer').outerHeight();
            if (heightToSet < bodyHeight) {
                heightToSet = bodyHeight;
            }
            $('.app-sidebar-menu').height(heightToSet);
        });
        return () => {
            window.removeEventListener("resize", () => {

            })
        }
    }, []);

    return (
        <div className='app-sidebar flex-column'>
            <div className='aside-logo flex-column-auto px-6'>
                {aside.theme === 'dark' && (
                    <Link to='/'>
                        <img
                            alt='Logo'
                            className='h-25px logo'
                        />
                    </Link>
                )}
                {aside.theme === 'light' && (
                    <Link to='/'>
                        <img
                            alt='Logo'
                            className='h-25px logo'
                        />
                    </Link>
                )}
            </div>

            <div className='app-sidebar-menu overflow-hidden flex-column-fluid'>
                <AsideMenu />
            </div>

            <div className='aside-footer flex-column-auto pt-5 pb-4'>
                <div className='menu menu-column'>
                    {/*<AsideMenuItem*/}
                    {/*    to='/notifications'*/}
                    {/*    icon='alert-outline'*/}
                    {/*    title={T('App.Navigation.Notifications')}*/}
                    {/*    menuIcon='svg'*/}
                    {/*/>*/}
                    <div className='menu-item d-flex justify-content-center'>
                        <button type='button' className="current-user">
                            <div className="user-thumbnail-box">
                                <div className="avatar-container user-thumbnail thumbnail-rounded">
                                    <span>IG</span>
                                </div>
                                <div className="source-badge user-online-status"></div>
                            </div>
                        </button>
                    </div>
                </div>
            </div>
        </div>
    )
}

export { AsideDefault }
