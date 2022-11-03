import React from 'react'
import { AsideMenuItem  } from './AsideMenuItem'
import { T } from '../../../utils/Utils'

export function AsideMenuMain() {
    return (
        <>
            <AsideMenuItem
                to='/'
                icon='chat-outline'
                title={T('App.Navigation.Dashboard')}
                menuIcon='svg'
            />
            <AsideMenuItem
                to='/contacts'
                icon='book-contacts-outline'
                title={T('App.Navigation.Contacts')}
                menuIcon='svg'
            />
            <AsideMenuItem
                to='/reports'
                icon='arrow-trending-lines-outline'
                title={T('App.Navigation.Reports')}
                menuIcon='svg'
            />
            <AsideMenuItem
                to='/campaings'
                icon='megaphone-outline'
                title={T('App.Navigation.Campaings')}
                menuIcon='svg'
            />
            <AsideMenuItem
                to='/settings'
                icon='settings-outline'
                title={T('App.Navigation.Settings')}
                menuIcon='svg'
            />
        </>
    )
}
