import clsx from 'clsx';

const Thobber = (props) => {
    const show = props.show;
    const large = props.large;
    const flex = props.flex;
    const text = props.text;

    return (
        <div className={clsx('throbber',
            { 'show': show },
            { 'large': large })}>
            <div className={clsx({ 'throbber-flex': flex })}>
                <div>
                    <div className="throbber-content">
                        <div id="install-message">{ text }</div>
                        <div id="install-progress"></div>
                    </div>
                    <div className="spinner active">
                        <svg viewBox="0 0 64 64">
                            <circle className="circle" cx="32" cy="32" r="29" fill="none" strokeWidth="1"></circle>
                        </svg>
                    </div>
                </div>
            </div>
        </div>
    )
}
export default Thobber