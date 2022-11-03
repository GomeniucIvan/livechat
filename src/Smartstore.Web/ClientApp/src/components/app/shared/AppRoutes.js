import Index from '../../app/conversation/Index'
import Install from './Install';

const AppRoutes = [
    {
        index: true,
        path: '/',
        element: <Index />
    },
    {
        path: '/install',
        element: <Install />
    }
];

export default AppRoutes;
