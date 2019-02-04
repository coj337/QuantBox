import React, { Component } from 'react';
import { Col } from 'react-bootstrap';
import Toggle from 'react-toggle';
import "react-toggle/style.css";
import { ExchangeChooser } from './ExchangeChooser';

export class ArbitrageSettings extends Component {
    displayName = ArbitrageSettings.name

    constructor(props) {
        super(props);

        this.state = {
            tradingEnabled: false
        };

        this.handleTradingEnabledChange = this.handleTradingEnabledChange.bind(this);
    }

    componentDidMount() {

    }

    handleTradingEnabledChange(e) {
        this.setState({
            tradingEnabled: e.target.checked
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
                        <ExchangeChooser />
                    </Col>
                </div>
            </Col>
        );
    }
}