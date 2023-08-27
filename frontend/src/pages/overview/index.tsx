import useGetDatasources from "@/hooks/use-get-datasources";
import useGetFrontendsBackends from "@/hooks/use-get-frontends-backends";
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
        <DatasourceCard title="Caches" datasourceList={ccoConfigs.caches} />

        <DatasourceCard title="Databases" datasourceList={ccoConfigs.databases} />

        <DatasourceCard title="Queues" datasourceList={ccoConfigs.queues} />
      </OuterCard>
    </main>
  );
};

export default Dashboard;
