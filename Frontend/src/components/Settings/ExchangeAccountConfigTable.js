import React, { Component } from 'react';
import { AccountConfig } from './AccountConfig';
import './AccountConfig.css';

export class ExchangeAccountConfigTable extends Component {
    displayName = ExchangeAccountConfigTable.name

    constructor(props) {
        super(props);

        this.state = {
            exchangeConfigs: []
        };
    }

    componentDidMount() {
        fetch("/Market/Settings")
            .then(res => res.json())
            .then(
                (result) => {
                    this.setState({
                        exchangeConfigs: result,
                    });
                },
                (error) => {
                    console.log(error);
                }
            );
    }

    render() {
        return (
            <div className="configTable">
                <div className="configTableTitle">
                    <span className="configTableTitleHeading">
                        Exchange
                    </span>
                    <span className="configTableTitleHeading">
                        Public Key
                    </span>
                    <span className="configTableTitleHeading">
                        Private Key
                    </span>
                </div>

                {this.state.exchangeConfigs.length > 0 ?
                    this.state.exchangeConfigs.map(function (config, i) {
                        return <AccountConfig key={i} name={config.name} publicKey={config.publicKey} />
                    }) :
                    <AccountConfig name="Binance" disabled="true" />
                }
                
                <AccountConfig name="BTCMarkets" disabled="true" />
                {/*<button id="addAccount"> + </button>*/}
            </div>
        );
    }
}