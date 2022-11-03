import React, { Component } from 'react';
import { get } from './HttpClient';
import { Loading } from './Loading';
import lang, { setResources } from './Translate.Languages';
import { isNullOrEmpty, stringFormat } from './Utils';


class Translate extends Component {
    constructor(props) {
        super(props);
        this.state = {
            loading: isNullOrEmpty(lang) || !lang.initialized
        };
    }

    componentDidMount() {
        if (isNullOrEmpty(lang) || !lang.initialized) {
            this.populateComponent();
        }
    }

    render() {
        if (this.state.loading) {
            return <Loading type={this.props.type} />
        }

        const text = this.props['text'];
        const args = this.props['args'];
        let resource = lang.resources.find((o) => { return o["ResourceName"] === text });
        if (isNullOrEmpty(resource)) {
            resource = text;
        }
        return stringFormat(resource, args);
    }

    async populateComponent() {
        const data = await get(`api/resources`);

        if (data.IsValid) {
            setResources(data.Data)
            this.setState({ loading: false });
        }
    }
}
export default Translate
