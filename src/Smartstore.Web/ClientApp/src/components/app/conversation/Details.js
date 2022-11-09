import DetailsBody from "./DetailsBody"
import DetailsFooter from "./DetailsFooter"
import DetailsHeader from "./DetailsHeader"

const Details = (props) => {
    return (
        <div className='conversation-details-wrap'>
            {<DetailsHeader />}
            <div className='conv-body'>
                {<DetailsBody />}
            </div>
            {<DetailsFooter />}
        </div>
    )
}

export default Details