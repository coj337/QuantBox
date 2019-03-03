import React, { Component } from 'react';
import Axios from 'axios';
import { toast } from 'react-toastify';
import './TemplatePreview.css';

export class TemplatePreview extends Component {
    displayName = TemplatePreview.name

    constructor(props) {
        super(props);

        this.state = {
            name: props.name,
            description: props.description
        };

        this.createBotFromTemplate = this.createBotFromTemplate.bind(this);
    }

    createBotFromTemplate(template) {
        Axios.post('/Bot/Create', { template: template })
            .then((response) => {
                this.setState({
                    template: template
                });
            })
            .catch((error) => {
                if (error.response.data) {
                    toast.error(error.response.data);
                }
                else {
                    toast.error("Couldn't create a new bot. (" + error.response.status + " " + error.response.statusText + ")");
                }
            });
    }

    render() {
        return (
            <div className="templatePreview darkerContainer" onClick={() => this.createBotFromTemplate(this.state.name)}>
                <h4 className="botTitle">{this.state.name}</h4>

                <p className="templateDescription">{this.state.description}</p>
            </div>
        );
    }
}