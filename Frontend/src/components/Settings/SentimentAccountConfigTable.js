import React, { Component } from 'react';
import { AccountConfig } from './AccountConfig';

export class SentimentAccountConfigTable extends Component {
    displayName = SentimentAccountConfigTable.name

    render() {
        return (
            <div className="configTable">
                <div className="configTableTitle">
                    <span className="configTableTitleHeading">
                        Platform
                    </span>
                    <span className="configTableTitleHeading">
                        Public Key
                    </span>
                    <span className="configTableTitleHeading">
                        Private Key
                    </span>
                </div>

                <AccountConfig name="Twitter" disabled="true" /> 
                <AccountConfig name="Reddit" disabled="true" />
            </div>
        );
    }
}