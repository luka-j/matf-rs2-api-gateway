import useGetDatasources from "@/hooks/use-get-datasources";
import { Typography } from "@/components/ui/typography";
import PageLoader from "@/components/page-loader";

import DatasourceCard from "./components/datasource-card";
import OuterCard from "./components/outer-card";

const Datasources = () => {
  const { data: ccoConfigs, isLoading: isLoadingCCOConfigs } = useGetDatasources();

  if (isLoadingCCOConfigs) return <PageLoader />;

  if (!ccoConfigs) return <div>Something went wrong</div>;

  return (
    <div className="container mt-8 flex flex-col gap-8 pb-8">
      <div className="flex justify-center">
        <Typography variant="h1">Datasources</Typography>
      </div>

      <OuterCard datasourceType="Caches">
        {ccoConfigs.caches.map((cache) => (
          <DatasourceCard
            datasourceType="caches"
            key={cache.datasource.connectionString}
            datasource={cache}
          />
        ))}
      </OuterCard>

      <OuterCard datasourceType="Databases">
        {ccoConfigs.databases.map((database) => (
          <DatasourceCard
            datasourceType="databases"
            key={database.datasource.connectionString}
            datasource={database}
          />
        ))}
      </OuterCard>

      <OuterCard datasourceType="Queues">
        {ccoConfigs.queues.map((queue) => (
          <DatasourceCard
            datasourceType="queues"
            key={queue.datasource.connectionString}
            datasource={queue}
          />
        ))}
      </OuterCard>
    </div>
  );
};

export default Datasources;
