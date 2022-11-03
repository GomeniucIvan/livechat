import { useEffect, useState} from 'react'
import { get } from "../../utils/HttpClient";
import { useLocation } from 'react-router-dom';
import { setHeaderData } from '../../utils/Utils';
import Login from '../identity/Login';
import Layout from './Layout';
import { Loading } from '../../utils/Loading';
import LauncherWrapper from '../../launcher/LauncherWrapper';
import Install from './Install';


const Document = () => {
    let [loading, setLoading] = useState(true);
    let [model, setModel] = useState(null);
    let [isRegistered, setIsRegistered] = useState(false);
    let [isInstalled, setIsInstalled] = useState(false);
    const location = useLocation();

    useEffect(() => {
        const PopulateComponent = async () => {
            const installResponse = await get(`/api/install`);

            if (installResponse.IsValid) {
                const installData = installResponse.Data;
                setIsInstalled(installData.IsInstalled);

                if (!installData.IsInstalled && location.pathname.toLocaleLowerCase() !== '/install') {
                    window.history.pushState({}, "", '/install');
                }
            } 

            if (installResponse.Data.IsInstalled) {
                const response = await get(`/api/startup`);
                const data = response.Data;

                if (response.IsValid) {
                    setModel(data);
                    setIsRegistered(data.IsRegistered);
                    setHeaderData(data);

                    if (!data.IsRegistered && location.pathname.toLocaleLowerCase() !== '/login') {
                        window.history.pushState({}, "", '/login');
                    }

                    if ((installResponse.Data.IsInstalled.IsInstalled && location.pathname.toLocaleLowerCase() === '/install') ||
                        (data.IsRegistered && location.pathname.toLocaleLowerCase() === '/login')) {
                        window.history.pushState({}, "", '/');
                    }
                }
            }

            setLoading(false);
        }

        PopulateComponent();
    }, []);

    const onLogin = () => {
        let currentModel = model;
        currentModel.IsRegistered = true;
        setModel(currentModel);
        setIsRegistered(true);
        window.history.pushState({}, "", '/');
    }

    return (
        loading ? <Loading /> : !isInstalled ? <Install /> : (!isRegistered ? <Login onLogin={onLogin} /> : (<><Layout /> <LauncherWrapper /></>))
    );
}

export default Document;
