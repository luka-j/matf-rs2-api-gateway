import { useUserStore } from "@/stores/userStore";

const Dashboard = () => {
  const currentUser = useUserStore((state) => state.currentUser);

  return (
    <div>
      <h1>Welcome,</h1>
      <p>{JSON.stringify(currentUser, null, 2)}</p>
    </div>
  );
};

export default Dashboard;
