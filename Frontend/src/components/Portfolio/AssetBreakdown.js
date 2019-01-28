import React, { Component } from 'react';
import './PortfolioOverview.css';
import '../../theme.css';

export class AssetBreakdown extends Component {
    displayName = AssetBreakdown.name

    constructor(props) {
        super(props);

        this.state = {
            currency: props.currency,
            symbol: props.symbol
        };
    }

    render() {
        return (
            <div className="assetBreakdown darkestContainer">
                <div className="left">
                    <img className="breakdownIcon" src={`/img/CryptoIcons/${this.state.symbol}.svg`} alt={`${this.state.symbol}-logo`}/> {this.state.currency}
                </div>
                <div className="right assetBreakdownValue">
                    <div>0.12345678</div>
                    <div className="fadedText">$123.45</div>
                </div>
            </div>
        );
    }
}
