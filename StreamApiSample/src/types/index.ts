export interface StreamingRequest {
    id: string;
    data: any;
}

export interface StreamingResponse {
    id: string;
    status: string;
    result: any;
}

export interface ErrorResponse {
    id: string;
    error: string;
}