import React, { Component } from 'react';
import { Col } from 'react-bootstrap';
import Toggle from 'react-toggle';
import "react-toggle/style.css";
import { ExchangeChooser } from './ExchangeChooser';
import Axios from 'axios';
import { toast } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.min.css';

export class ArbitrageSettings extends Component {
    displayName = ArbitrageSettings.name

    constructor(props) {
        super(props);

        this.state = {
            tradingEnabled: false,
            botId: props.botId
        };

        this.handleTradingEnabledChange = this.handleTradingEnabledChange.bind(this);
    }

    componentDidMount() {
        Axios.get('/Settings/GetTradingState?botId=' + this.state.botId)
            .then((response) => {
                this.setState({
                    tradingEnabled: response.data
                });
            })
            .catch((error) => {
                if (error.response.data) {
                    toast.error(error.response.data);
                }
                else {
                    toast.error("Couldn't get trading state. (" + error.response.status + " " + error.response.statusText + ")");
                }
            });
    }

    handleTradingEnabledChange(e) {
        Axios.post('/Settings/SetTradingState', { botId: this.state.botId, state: e.target.checked })
            .then((response) => {
                this.setState({
                    tradingEnabled: e.target.checked
                });
            })
            .catch((error) => {
                if (error.response.data) {
                    toast.error(error.response.data);
                }
                else {
                    toast.error("Couldn't set trading state. (" + error.response.status + " " + error.response.statusText + ")");
                }
            });
    }

    render() {
        return (
            <Col xs={12}>
                <h4 className="subTitle">Settings</h4>
                <div id="arbitrageSettings" className="darkerContainer">
                    <Col xs={3}>
                        <span>
                            Trading <span>{this.state.tradingEnabled ? "Enabled" : "Disabled"}</span>
                        </span>

                        <Toggle
                            defaultChecked={this.state.tradingEnabled}
                            onChange={this.handleTradingEnabledChange}
                        />
                    </Col>

                    <Col xs={6}>
                        <ExchangeChooser botId={this.state.botId} />
                    </Col>
                </div>
            </Col>
        );
    }
}