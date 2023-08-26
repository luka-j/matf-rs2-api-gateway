import useGetDatasources from "@/hooks/use-get-datasources";
import useGetFrontendsBackends from "@/hooks/use-get-frontends-backends";
import { caches, databases, queues } from "@/mock/overview";
import { Typography } from "@/components/ui/typography";
import PageLoader from "@/components/page-loader";

import ConfigOverviewCard from "./components/config-overview-card";
import DatasourceCard from "./components/datasource-card";
import OuterCard from "./components/outer-card";

const Dashboard = () => {
  const { data: frontends, isLoading: isLoadingFrontends } = useGetFrontendsBackends("frontend");
  const { data: backends, isLoading: isLoadingBackends } = useGetFrontendsBackends("backend");
  const { data: ccoConfigs, isLoading: isLoadingCCOConfigs } = useGetDatasources();

  if (isLoadingFrontends || isLoadingBackends || isLoadingCCOConfigs) return <PageLoader />;

  if (!frontends || !backends || !ccoConfigs) return <div>Something went wrong</div>;

  return (
    <main className="mx-auto mt-8 max-w-7xl space-y-8 pb-8">
      <Typography variant="h1" className="text-center">
        API Gateway Dashboard
      </Typography>

      <OuterCard title="Config Overview" description="Here you can preview and edit your configs.">
        <ConfigOverviewCard
          title="Frontends"
          viewAllURL="/dashboard/frontends"
          frontBackList={frontends}
        />

        <ConfigOverviewCard
          title="Backends"
          viewAllURL="/dashboard/backends"
          frontBackList={backends}
        />
      </OuterCard>

      <OuterCard
        title="Datasources"
        description="Here you can preview and edit your datasources."
        viewAllURL="/dashboard/datasources"
      >
        <DatasourceCard
          title="Caches"
          upNum={5}
          totalNum={7}
          description="Here you can preview and edit your caches."
          datasourceList={caches}
        />

        <DatasourceCard
          title="Databases"
          upNum={3}
          totalNum={7}
          description="Here you can preview and edit your databases."
          datasourceList={databases}
        />

        <DatasourceCard
          title="Queues"
          upNum={1}
          totalNum={7}
          description="Here you can preview and edit your queues."
          datasourceList={queues}
        />
      </OuterCard>
    </main>
  );
};

export default Dashboard;
