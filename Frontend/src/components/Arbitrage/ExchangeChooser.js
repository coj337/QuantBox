import React, { Component } from 'react';
import Select from 'react-select';
import './ExchangeChooser.css';
import Axios from 'axios';
import { ToastContainer, toast } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.min.css';

export class ExchangeChooser extends Component {
    displayName = ExchangeChooser.name

    constructor(props) {
        super(props);

        this.state = {
            exchanges: [],
            selectedAccounts: [],
            accountsLoaded: false,
            botAccountsLoaded: false,
            botId: this.props.botId
        };
    }

    componentDidMount() {
        fetch("/Settings/Accounts")
            .then(res => res.json())
            .then(
                (result) => {
                    this.setState({
                        exchanges: result,
                        accountsLoaded: true
                    });
                },
                (error) => {
                    if (error.response.data) {
                        toast.error(error.response.data);
                    }
                    else {
                        toast.error("Couldn't get accounts. (" + error.response.status + " " + error.response.statusText + ")");
                    }
                }
        );

        Axios.get('/Settings/BotAccounts?botId=' + this.state.botId)
            .then((response) => {
                this.setState({
                    selectedAccounts: response,
                    botAccountsLoaded: true
                });
            })
            .catch((error) => {
                if (error.response.data) {
                    toast.error(error.response.data);
                }
                else {
                    toast.error("Couldn't get bot accounts. (" + error.response.status + " " + error.response.statusText + ")");
                }
            });
    }

    updateSelectedAccount(exchange, account) {
        Axios.post('/Settings/UpdateBotAccount', {
            BotId: this.state.botId,
            Exchange: exchange,
            Account: account
        })
            .then((response) => {
                this.setState(prevState => ({
                    selectedAccounts: {
                        ...prevState.selectedAccounts,
                        [exchange]: account
                    }
                }));
            toast.success("Account updated");
        })
        .catch((error) => {
            if (error.response.data) {
                toast.error(error.response.data);
            }
            else {
                toast.error("Couldn't update account. (" + error.response.status + " " + error.response.statusText + ")");
            }
        });
    }

    render() {
        if (this.state.botAccountsLoaded && this.state.accountsLoaded) {
            var options = {};
            for (var i = 0; i < this.state.exchanges.length; i++) {
                if (!(this.state.exchanges[i].name in options)) {
                    options[this.state.exchanges[i].name] = [];
                }
                var nickname = this.state.exchanges[i].nickname;
                if (this.state.exchanges[i].simulated) {
                    nickname += " (Simulated)";
                }
                options[this.state.exchanges[i].name].push({ value: this.state.exchanges[i].nickname, label: nickname });
            }
        }

        return (
            <div>
                {this.state.botAccountsLoaded && this.state.accountsLoaded ?
                    Object.keys(options).map((key, i) => {
                        var defaultValue = this.state.selectedAccounts[key] ? this.state.selectedAccounts[key] : "Choose an account";
                        return <div key={i}>
                            <span>{key}</span>
                            <Select
                                className="exchangeSelect"
                                placeholder="Choose an account"
                                isLoading={!this.state.botAccountsLoaded || !this.state.accountsLoaded}
                                isDisabled={!this.state.botAccountsLoaded || !this.state.accountsLoaded}
                                isSearchable={true}
                                options={options[key]}
                                onChange={value => this.updateSelectedAccount(value)}
                                value={defaultValue}
                            />
                        </div>
                    }) :
                    ""
                }

                <ToastContainer />
            </div>
        );
    }
}