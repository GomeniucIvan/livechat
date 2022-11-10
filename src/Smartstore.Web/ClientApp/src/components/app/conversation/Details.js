import DetailsBody from "./DetailsBody"
import DetailsFooter from "./DetailsFooter"
import DetailsHeader from "./DetailsHeader"

const Details = (props) => {
    
    return (
        <div className='conversation-details-wrap'>
            {<DetailsHeader companyGuestCustomer={props.companyGuestCustomer} />}
            {<DetailsBody companyGuestCustomer={props.companyGuestCustomer} />}
            {<DetailsFooter companyGuestCustomer={props.companyGuestCustomer} />}
        </div>
    )
}

export default Details