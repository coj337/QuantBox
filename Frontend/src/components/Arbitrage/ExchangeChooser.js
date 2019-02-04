import React, { Component } from 'react';
import Select from 'react-select';
import './ExchangeChooser.css';

export class ExchangeChooser extends Component {
    displayName = ExchangeChooser.name

    constructor(props) {
        super(props);

        this.state = {
            exchanges: [],
            exchangesLoaded: false
        };
    }

    componentDidMount() {
        fetch("/Settings/Accounts")
            .then(res => res.json())
            .then(
                (result) => {
                    this.setState({
                        exchanges: result,
                        exchangesLoaded: true
                    });
                },
                (error) => {
                    console.log(error);
                }
            );
    }

    render() {
        if (this.state.exchangesLoaded) {
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
                {this.state.exchangesLoaded ?
                    Object.keys(options).map((key, i) => {
                        return <div key={i}>
                            <span>{key}</span>
                            <Select
                                className="exchangeSelect"
                                placeholder="Choose an account"
                                isLoading={!this.state.exchangesLoaded}
                                isDisabled={!this.state.exchangesLoaded}
                                isSearchable={true}
                                options={options[key]}
                            />
                        </div>
                    }) :
                    ""
                }
            </div>
        );
    }
}