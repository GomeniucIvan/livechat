import { useEffect, useState } from "react";
import { postLauncher } from "../../utils/HttpClient";
import { isNullOrEmpty } from "../../utils/Utils";
import { HubConnectionBuilder } from '@microsoft/signalr';

const DetailsBody = (props) => {
    const [connection, setConnection] = useState(null);
    let [messageList, setMessageList] = useState([]);
    let [loading, setLoading] = useState(true);

    useEffect(() => {
        const newConnection = new HubConnectionBuilder()
            .withUrl("/chatHub")
            //.configureLogging(signalR.LogLevel.None)
            .build();

        setConnection(newConnection);

        const PopulateComponent = async () => {
            let response = await postLauncher(`/api/messages`);

            if (response && response.IsValid) {
                setMessageList(response.Data);
            }

            setLoading(false);
        }

        PopulateComponent();
    }, []);

    useEffect(() => {
        if (connection) {
            console.log(connection)

            connection.start().then(result => {
                    console.log('Connected!');

                    connection.on('company_1_new_message', message => {
                        console.log(message)
                    });
            }).catch(e => console.log('Connection failed: ', e));
        }
    }, [connection]);

    return (
        <>
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
        </>
    )
}

export default DetailsBody