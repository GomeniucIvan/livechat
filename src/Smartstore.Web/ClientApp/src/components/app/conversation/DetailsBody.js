import { useEffect, useRef, useState } from "react";
import { postLauncher } from "../../utils/HttpClient";
import { isNullOrEmpty } from "../../utils/Utils";
import { HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import { Loading } from "../../utils/Loading";

let messageArrayList = []; //todo ?! setMessageList reload component!!

const DetailsBody = (props) => {
    let [messageList, setMessageList] = useState([]);
    let [loading, setLoading] = useState(true);
    const scrollRef = useRef(null);

    useEffect(() => {
        const connection = new HubConnectionBuilder()
            .withUrl("/chatHub")
            .withAutomaticReconnect()
            .configureLogging(LogLevel.Debug)
            .build();

        connection.start().catch(function (err) {
            console.log('Hub Error' + err);
        });

        connection.on(`company_1_new_message`, function (message) {
            const incMessage = {
                Id: message.id,
                CompanyCustomerId: message.companyCustomerId,
                CompanyGuestCustomerId: message.companyGuestCustomerId,
                CompanyId: message.companyId,
                CreatedOnUtc: message.createdOnUtc,
                IconUrl: message.iconUrl,
                Message: message.message,
                Sent: message.sent
            }

            setMessageList([...messageArrayList, incMessage]);
            messageArrayList = messageList;

            setTimeout(() => {
                if (scrollRef.current) {
                    scrollRef.current.scrollTop = scrollRef.current.scrollHeight;
                }
            }, 50);
        });

        const PopulateComponent = async () => {
            let response = await postLauncher(`/api/messages`);

            if (response && response.IsValid) {
                setMessageList(response.Data);
                messageArrayList = response.Data;
            }

            setLoading(false);

            setTimeout(() => {
                if (scrollRef.current) {
                    scrollRef.current.scrollTop = scrollRef.current.scrollHeight;
                }
            }, 500);
        }

        PopulateComponent();
    }, []);

    if (loading) {
        return <Loading />
    }

    return (
        <div className='conv-body' ref={scrollRef}>
            {messageList.map(message => {
                return (
                    <div key={message.Id} className={`conv-message ${isNullOrEmpty(message.CompanyCustomerId) ? 'guest-message' : 'customer-message'}`}>
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
                )
            })}
        </div>
    )
}

export default DetailsBody