export interface Asset {
    id: number;
    name: string;
    model: string;
    serial: string;
    user: number;
    user_first_name: string;
    user_last_name: string;
    next_user?: number;
    purchase_date: string;
    warranty: number;
    type: number;
    status: number;
    description?: string;
}