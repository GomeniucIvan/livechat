import { AsideDefault } from "./aside/AsideDefault";
import AppRoutes from './AppRoutes';
import { Route, Routes } from 'react-router-dom';

const Layout = () => {
    return (
        <>
            <div className='root-wrapper d-flex flex-row flex-column-fluid'>
                <AsideDefault />
                <div className='app-content columns'>
                    <Routes>
                        {AppRoutes.map((route, index) => {
                            const { element, ...rest } = route;
                            return <Route key={index} {...rest} element={element} />;
                        })}
                    </Routes>
                </div>
            </div>
        </>
    )
}

export default Layout;
