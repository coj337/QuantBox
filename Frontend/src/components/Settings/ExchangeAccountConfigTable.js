import React, { Component } from 'react';
import { AccountConfig } from './AccountConfig';
import './AccountConfig.css';
import { Input } from '../Forms/Input';

import Modal from 'react-awesome-modal';
import Select from 'react-select';
import Axios from 'axios';
import { ToastContainer, toast } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.min.css';

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
        this.deleteCreds = this.deleteCreds.bind(this);
    }

    componentDidMount() {
        fetch("/Settings/SupportedExchanges")
            .then(res => res.json())
            .then(
                (result) => {
                    var exchangeOptions = result.map(function (exchange, i) {
                        return { value: exchange, label: exchange }
                    });
                    this.setState({
                        supportedExchanges: exchangeOptions,
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
        var publicKey = document.getElementsByName("publicKey")[0].value;
        var privateKey = document.getElementsByName("privateKey")[0].value;
        var exchange = document.getElementsByName("exchange")[0].value;
        var nickname = document.getElementsByName("nickname")[0].value;

        Axios.post('/Settings/AddExchangeConfig', {
            Name: exchange,
            NickName: nickname,
            PublicKey: publicKey,
            PrivateKey: privateKey
        })
        .then((response) => {
            this.setState(previousState => ({
                exchangeConfigs: [...previousState.exchangeConfigs, { name: exchange, nickname: nickname, publicKey: publicKey, privateKey: privateKey }],
                modalVisible: false
            }));
        })
        .catch((error) => {
            this.setState({
                modalVisible: false
            });
            toast.error(error.response.data);
        });

        //Clear the boxes now
        document.getElementsByName("publicKey")[0].value = "";
        document.getElementsByName("privateKey")[0].value = "";
        document.getElementsByName("nickname")[0].value = "";
    }

    deleteCreds(name, key) {
        Axios.post('/Settings/RemoveExchangeConfig', {
            Name: name,
            PublicKey: key
        })
        .then((response) => {
            this.setState({
                exchangeConfigs: this.state.exchangeConfigs.filter((config, i) => config.name !== name || config.publicKey !== key)
            });
        })
        .catch((error) => {
            toast.error(error.response.data);
        });
    }

    render() {
        return (
            <div className="configTable">
                <div className="configTableTitle">
                    <span className="configTableTitleHeading">
                        Nickname
                    </span>
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
                    this.state.exchangeConfigs.map((config, i) => {
                        return <AccountConfig key={i} name={config.name} nickname={config.nickname} publicKey={config.publicKey} onDelete={this.deleteCreds} />
                    }) :
                    <div className="m-l-20 m-t-10 fadedText">No exchanges, click the button below to add one!</div>
                }
                
                <button id="addAccount" className="darkerContainer" onClick={() => this.openModal()}> + </button>
                <Modal visible={this.state.modalVisible} width="400" height="300" effect="fadeInUp" onClickAway={() => this.closeModal()}>
                    <div id="exchangeConfig" className="customModal">
                        <Select
                            id="exchangeSelect"
                            className="modalSelect"
                            placeholder="Choose an exchange"
                            isLoading={!this.state.exchangesLoaded}
                            isDisabled={!this.state.exchangesLoaded}
                            isSearchable={true}
                            name="exchange"
                            options={this.state.supportedExchanges}
                        />
                        <Input
                            name="nickname"
                            label="Nickname"
                            locked={!this.state.exchangesLoaded}
                        />
                        <Input
                            name="publicKey"
                            label="Public Key"
                            locked={!this.state.exchangesLoaded}
                        />
                        <Input
                            name="privateKey"
                            label="Private Key"
                            locked={!this.state.exchangesLoaded}
                        />
                        <label>Simulated</label> <input type="checkbox" name="simulated" /><br/>

                        <button className="m-t-10" onClick={this.saveCreds}>Save</button>
                    </div>
                </Modal>

                <ToastContainer/>
            </div>
        );
    }
}