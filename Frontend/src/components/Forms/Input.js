import React from "react";
import "./input.css";

export class Input extends React.Component {
    constructor(props) {
        super(props);

        this.state = {
            active: (props.locked && props.active) || false,
            value: props.value || "",
            label: props.label || "Label",
            name: props.name || ""
        };
    }

    changeValue(event) {
        const value = event.target.value;
        this.setState({ value, error: "" });
    }

    render() {
        const { active, label, name } = this.state;
        const { locked } = this.props;
        const fieldClassName = `field ${(locked ? active : active) && "active"} ${locked && !active && "locked"}`;

        return (
            <div className={fieldClassName}>
                <input
                    name={name}
                    type="text"
                    placeholder={label}
                    onChange={this.changeValue.bind(this)}
                    onFocus={() => !locked && this.setState({ active: true })}
                    onBlur={() => !locked && this.setState({ active: false })}
                />
            </div>
        );
    }
}