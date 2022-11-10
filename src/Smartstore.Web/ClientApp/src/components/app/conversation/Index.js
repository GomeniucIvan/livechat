import { useState } from "react";
import { KTSVG } from '../../utils/Utils'
import Translate from '../../utils/Translate'
import Details from "./Details";

const Index = (props) => {
    const [loading, setLoading] = useState(false);

    const companyGuestCustomer = {
        Id: 1,
        FirstName: 'test',
        LastName: 'last',
        FullName: 'test last',
        Description: 'Company',
        LastMessage: 'Last message',
        LastMessageIncomeTimeAgoDisplay: '3h',
        CompanyId: 1
    }

    return (
        <>
            <section className='conversation-page'>
                <div className='conversations-list-wrap'>
                    <div className='conversations-list-wrap-head d-flex flex-row align-items-center'>
                        <div className='flex flex-row items-center '>
                            <KTSVG width='30' height='30' icon='hamburger' className='svg-icon-2' />
                            <span className='conversations-list-wrap-head-status'>
                                <Translate text='App.Conversation.Status.All' />
                            </span>
                        </div>
                    </div>
                    <div className='conversations-list'>
                        <div className='conversation-item'>
                            <div className='d-flex w-100'>
                                <span className="current-user">
                                    <div className="user-thumbnail-box">
                                        <div className="avatar-container user-thumbnail thumbnail-rounded">
                                            <span>IG</span>
                                        </div>
                                        <div className="source-badge user-online-status"></div>
                                    </div>
                                </span>
                                <span className='current-user-details'>
                                    <span>
                                        <div className='current-user-initials'>
                                            {companyGuestCustomer.FullName}
                                        </div>
                                        <div className='current-user-description'>
                                            {companyGuestCustomer.Description}
                                        </div>
                                    </span>
                                    <span>
                                        {companyGuestCustomer.LastMessageIncomeTimeAgoDisplay}
                                    </span>
                                </span>
                            </div>

                            <div className='reply-text'>

                            </div>
                            <div className='sent-text'>
                                {companyGuestCustomer.LastMessage}
                            </div>
                        </div>
                    </div>
                </div>

                {<Details companyGuestCustomer={companyGuestCustomer} />}

                <div className='conversation-details-summary'>

                </div>
            </section>
        </>
    )
}

export default Index