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
        fetch("/Market/Tickers")
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
            Header: 'Pair',
            accessor: 'pair'
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
               <Widget
                   title={"Market Watch"}
                   minH={2}
                   minW={2}
                   currentH={2}
                   currentW={2}
                   x={10}
                   y={0}
               >
                   <ReactTable
                       data={marketData}
                       columns={columns}
                       minRows={1}
                       showPagination={false}
                   />
                </Widget>
            );
        }
    }
}