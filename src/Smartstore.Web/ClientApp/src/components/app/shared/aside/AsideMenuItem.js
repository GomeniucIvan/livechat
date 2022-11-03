import React from 'react'
import clsx from 'clsx'
import { NavLink } from 'react-router-dom'
import { isNullOrEmpty, KTSVG } from '../../../utils/Utils'
import ReactTooltip from 'react-tooltip'

const AsideMenuItem = ({
    children,
    to,
    title,
    icon,
    fontIcon,
    menuIcon,
}) => {

    return (
        <div className='menu-item'>
            <NavLink data-for={`${icon}-svg`} className={`menu-link`} to={to} title={title} end>
                {icon && menuIcon === 'svg' && (
                    <span className='menu-icon'>                      
                        <KTSVG icon={icon} className='svg-icon-2' />
                    </span>
                )}

                {fontIcon && menuIcon === 'font' && <i className={clsx('bi fs-3', fontIcon)}></i>}
            </NavLink>
            {!isNullOrEmpty(title) &&
                <ReactTooltip id={`${icon}-svg`} type='dark' effect='solid'>
                    {title}
                </ReactTooltip>
            }
            
            {children}
        </div>
    )
}

export { AsideMenuItem }
