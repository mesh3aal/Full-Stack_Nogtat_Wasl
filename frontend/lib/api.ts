import axios from "axios";

export const api = axios.create({
  baseURL: process.env.NEXT_PUBLIC_API_URL || "https://localhost:5001",
  withCredentials: true,
  headers: {
    "Content-Type": "application/json",
  },
});
