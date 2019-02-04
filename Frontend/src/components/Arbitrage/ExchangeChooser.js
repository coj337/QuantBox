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
        fetch("/Settings/SupportedExchanges")
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
        const options = [
            { value: 'test', label: 'test' }
        ]

        return (
            <div>
                {this.state.exchanges.map((exchange, i) => {
                        return <div key={i}>
                            <span>{exchange}</span>
                            <Select
                                className="exchangeSelect"
                                placeholder="Choose an account"
                                isLoading={!this.state.exchangesLoaded}
                                isDisabled={!this.state.exchangesLoaded}
                                isSearchable={true}
                                options={options}
                            />
                        </div>
                    })
                }
            </div>
        );
    }
}