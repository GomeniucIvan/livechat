import { Component } from "react";

export class Loading extends Component {
    render() {
        if (this.props.type === 'button') {
            return (
                <svg className="btn-resource-loading" viewBox="0 0 44 44" xmlns="http://www.w3.org/2000/svg" stroke="#fff">
                    <g fill="none" fillRule="evenodd" strokeWidth="2">
                        <circle cx="22" cy="22" r="1">
                            <animate attributeName="r" begin="0s" dur="1.8s" values="1; 20" calcMode="spline" keyTimes="0; 1" keySplines="0.165, 0.84, 0.44, 1" repeatCount="indefinite"></animate>
                            <animate attributeName="stroke-opacity" begin="0s" dur="1.8s" values="1; 0" calcMode="spline" keyTimes="0; 1" keySplines="0.3, 0.61, 0.355, 1" repeatCount="indefinite"></animate>
                        </circle>
                        <circle cx="22" cy="22" r="1">
                            <animate attributeName="r" begin="-0.9s" dur="1.8s" values="1; 20" calcMode="spline" keyTimes="0; 1" keySplines="0.165, 0.84, 0.44, 1" repeatCount="indefinite"></animate>
                            <animate attributeName="stroke-opacity" begin="-0.9s" dur="1.8s" values="1; 0" calcMode="spline" keyTimes="0; 1" keySplines="0.3, 0.61, 0.355, 1" repeatCount="indefinite"></animate>
                        </circle>
                    </g>
                </svg>
                )
        }

        if (this.props.type === 'text') {
            return (
                <svg className="resource-loading" viewBox="0 0 44 44" xmlns="http://www.w3.org/2000/svg" stroke="#4b535c">
                    <g fill="none" fillRule="evenodd" strokeWidth="2">
                        <circle cx="20" cy="20" r="1">
                            <animate attributeName="r" begin="0s" dur="1.8s" values="1; 20" calcMode="spline" keyTimes="0; 1" keySplines="0.165, 0.84, 0.44, 1" repeatCount="indefinite"></animate>
                            <animate attributeName="stroke-opacity" begin="0s" dur="1.8s" values="1; 0" calcMode="spline" keyTimes="0; 1" keySplines="0.3, 0.61, 0.355, 1" repeatCount="indefinite"></animate>
                        </circle>
                        <circle cx="20" cy="20" r="1">
                            <animate attributeName="r" begin="-0.9s" dur="1.8s" values="1; 20" calcMode="spline" keyTimes="0; 1" keySplines="0.165, 0.84, 0.44, 1" repeatCount="indefinite"></animate>
                            <animate attributeName="stroke-opacity" begin="-0.9s" dur="1.8s" values="1; 0" calcMode="spline" keyTimes="0; 1" keySplines="0.3, 0.61, 0.355, 1" repeatCount="indefinite"></animate>
                        </circle>
                    </g>
                </svg>
            )
        }

        return (
            <span className="loading-wrapper">
                <div className="spinner active w-100">
                    <svg style={{ width: '50px', height: '50px', opacity: '0.8' }} viewBox="0 0 64 64">
                        <circle className="circle" cx="32" cy="32" r="28" fill="none" strokeWidth="2">
                        </circle>
                    </svg>
                </div>
            </span>
        )
    }
}