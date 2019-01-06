import React, { Component } from 'react';
import Select from 'react-select';
import { Widget } from './Widget';

export class Output extends Component {
    state = {
        selectedOption: null
    }
    handleChange = (selectedOption) => {
        this.setState({ selectedOption });
    }

    render() {
        const options = [
            { value: 'all', label: 'All' },
            { value: 'system', label: 'System' },
            { value: 'indicator', label: 'Indicator' },
            { value: 'bot', label: 'Bot' }
        ];
        const { selectedOption } = this.state;
        return (
            <Widget
                title={"Output"}
                minH={2}
                minW={2}
                currentH={2}
                currentW={2}
            >
                <Select
                    value={selectedOption}
                    onChange={this.handleChange}
                    options={options}
                    className="outputSelect"
                    defaultValue="All"
                />
            </Widget>
        );
    }
}