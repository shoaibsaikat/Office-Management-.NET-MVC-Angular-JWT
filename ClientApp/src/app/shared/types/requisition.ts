export interface Requisition {
    id: number;
    user: number;
    user_name: string;
    item_name: string;
    unit: string;
    total: number;
    approver: number;
    approver_name: string;
    distributor?: number;
    distributor_name?: string;
    approved?: boolean;
    distributed?: boolean;
    title: string;
    amount: number;
    comment?: string;
    date: string;
}