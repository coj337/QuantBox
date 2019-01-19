import React, { Component } from 'react';
import { Tab, Tabs, TabList, TabPanel } from 'react-tabs';
import "react-tabs/style/react-tabs.css";
import { ExchangeAccountConfigTable } from './ExchangeAccountConfigTable';
import { SentimentAccountConfigTable } from './SentimentAccountConfigTable';
import { WalletAccountConfigTable } from './WalletAccountConfigTable';

export class Settings extends Component {
    displayName = Settings.name

    constructor(props) {
        super(props);

        this.state = {

        };
    }

    componentDidMount() {
        
    }

    render() {
        return (
            <Tabs>
                <TabList>
                    <Tab>Exchanges</Tab>
                    <Tab>Sentiment Providers</Tab>
                    <Tab>Wallets</Tab>
                </TabList>

                <TabPanel>
                    <ExchangeAccountConfigTable />
                </TabPanel>
                <TabPanel>
                    <SentimentAccountConfigTable />
                </TabPanel>
                <TabPanel>
                    <WalletAccountConfigTable />
                </TabPanel>
            </Tabs>
        );
    }
}