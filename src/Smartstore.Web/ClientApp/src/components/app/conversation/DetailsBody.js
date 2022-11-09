import { useEffect, useState } from "react";
import { postLauncher } from "../../utils/HttpClient";
import { isNullOrEmpty } from "../../utils/Utils";

const DetailsBody = (props) => {
    let [messageList, setMessageList] = useState([]);
    let [loading, setLoading] = useState(true);

    useEffect(() => {
        const PopulateComponent = async () => {
            let response = await postLauncher(`/api/messages`);

            if (response && response.IsValid) {
                setMessageList(response.Data);
            }

            setLoading(false);
        }

        PopulateComponent();
    }, []);

    return (
        <>
            {messageList.map(message => {
                return (
                    <>
                        <div className={`conv-message ${isNullOrEmpty(message.CompanyCustomerId) ? 'guest-message' : 'customer-message'}`}>
                            {!isNullOrEmpty(message.CompanyCustomerId) &&
                                <>
                                <span className='message'>
                                    <span dangerouslySetInnerHTML={{ __html: message.Message }}></span>
                                </span>
                                <span className='message-icon'>
                                    <img src={message.IconUrl} />
                                </span>
                                </>
                            }

                            {isNullOrEmpty(message.CompanyCustomerId) &&
                                <>
                                <span className='message-icon'>
                                    <img src={message.IconUrl} />
                                </span>

                                <span className='message'>
                                    <span dangerouslySetInnerHTML={{ __html: message.Message }}></span>
                                </span>

                                </>
                            }
                        </div>
                    </>
                    )
            })}
        </>
    )
}

export default DetailsBody