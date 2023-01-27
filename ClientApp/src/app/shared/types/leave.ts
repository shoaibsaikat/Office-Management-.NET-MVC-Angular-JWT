export interface Leave {
    id: number;
    title: string;
    user: number;
    user_first_name: string;
    user_last_name: string;
    creation_date: string;
    approver: number;
    approved: boolean;
    approve_date: string;
    start_date: string;
    end_date: string;
    day_count: number;
    comment: string;
}