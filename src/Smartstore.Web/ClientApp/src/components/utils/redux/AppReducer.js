import { combineReducers } from 'redux';
import { companyGuestCostumerReducer } from './CompanyGuestCustomerReducer';

export const appReducer = combineReducers( {
    companyGuest: companyGuestCostumerReducer,
})