import React, { Component } from 'react';
import { AccountConfig } from './AccountConfig';

export class WalletAccountConfigTable extends Component {
    displayName = WalletAccountConfigTable.name

    render() {
        return (
            <div className="configTable">
                <AccountConfig name="Bitcoin" disabled="true"/>
                <AccountConfig name="Ethereum" disabled="true"/>
            </div>
        );
    }
}