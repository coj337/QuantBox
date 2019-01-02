import React, { Component } from 'react';

import ReactTable from "react-table";
import "react-table/react-table.css";
import { Widget } from './Widget';

export class MarketWatch extends Component {
    constructor(props) {
        super(props);
        this.state = {
            error: null,
            isLoaded: false,
            marketData: []
        };
    }

    componentDidMount() {
        fetch("/Prices/Tickers")
            .then(res => res.json())
            .then(
                (result) => {
                    this.setState({
                        isLoaded: true,
                        marketData: result
                    });
                },
                (error) => {
                    this.setState({
                        isLoaded: true,
                        error
                    });
                }
            );
    }

    render() {
        const columns = [{
            Header: 'Symbol',
            accessor: 'symbol'
        }, {
            Header: 'Bid',
            accessor: 'bid'
        }, {
            Header: 'Ask',
            accessor: 'ask'
        }];

        const { error, isLoaded, marketData } = this.state;
        if (error) {
            return <div>Error: {error.message}</div>;
        } else if (!isLoaded) {
            return <div>Loading...</div>;
        } else {
            return (
                <div>
                    <ReactTable
                        data={marketData}
                        columns={columns}
                        minRows={1}
                        showPagination={false}
                    />
                </div>
            );
        }
    }
}