import React, { Component } from 'react';
import Modal from 'react-awesome-modal';
import Select from 'react-select';
import { AccountConfig } from './AccountConfig';
import './AccountConfig.css';

export class ExchangeAccountConfigTable extends Component {
    displayName = ExchangeAccountConfigTable.name

    constructor(props) {
        super(props);

        this.state = {
            supportedExchanges: [],
            exchangeConfigs: [],
            modalVisible: false,
            exchangesLoaded: false
        };

        this.openModal = this.openModal.bind(this);
        this.closeModal = this.closeModal.bind(this);
        this.saveCreds = this.saveCreds.bind(this);
    }

    componentDidMount() {
        fetch("/Settings/SupportedExchanges")
            .then(res => res.json())
            .then(
                (result) => {
                    this.setState({
                        supportedExchanges: result,
                        exchangesLoaded: true
                    });
                },
                (error) => {
                    console.log(error);
                }
        );

        fetch("/Settings/ExchangeConfigs")
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

    openModal() {
        this.setState({
            modalVisible: true
        });
    }

    closeModal() {
        this.setState({
            modalVisible: false
        });
    }

    saveCreds() {
        //TODO: Find creds

        //TODO: Post creds to server

        this.setState(previousState => ({
            exchangeConfigs: [...previousState.exchangeConfigs, { name: "c", publicKey: "a", privateKey: "b" }],
            modalVisible: false
        }));
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
                    <div className="m-l-20 m-t-10 fadedText">No exchanges, click the button below to add one!</div>
                }
                
                <button id="addAccount" className="darkerContainer" onClick={() => this.openModal()}> + </button>
                <Modal visible={this.state.modalVisible} width="400" height="300" effect="fadeInUp" onClickAway={() => this.closeModal()}>
                    <div className="customModal">
                        <Select
                            className="exchangeSelect"
                            defaultValue="Choose an exchange"
                            isLoading={!this.state.exchangesLoaded}
                            isDisabled={!this.state.exchangesLoaded}
                            isSearchable={true}
                            name="exchange"
                            options={this.state.supportedExchanges}
                        />

                        <input id="publicKeyInput" type="text" /><br/>
                        <input id="privateKeyInput" type="text" /><br />

                        <button onClick={this.saveCreds}>Save</button>
                    </div>
                </Modal>
            </div>
        );
    }
}