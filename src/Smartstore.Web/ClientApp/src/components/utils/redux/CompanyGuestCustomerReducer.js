import { COMPANYGUESTCUSTOMER_TYPING } from "./AppReducer.Types";

const initialState = {
    companyguestcustomer: null,
    loaded: false
}

export const companyGuestCostumerReducer = (state = initialState, action) => {
    switch (action.type) {
        case COMPANYGUESTCUSTOMER_TYPING:
            return { ...state, companyguestcustomer: action.payload, loaded: true }
        default:
            return state
    }
}