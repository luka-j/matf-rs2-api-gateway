import { Datasource } from "@/types/cco-config";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { Typography } from "@/components/ui/typography";

interface IDatasourceCardProps {
  title: string;
  datasourceList: Datasource[];
}

const DatasourceCard = ({ title, datasourceList }: IDatasourceCardProps) => {
  return (
    <Card>
      <CardHeader>
        <CardTitle className="flex justify-between">
          {title}: {datasourceList.length}/{datasourceList.length} up
        </CardTitle>
        <CardDescription>Here you can preview and edit your {title.toLowerCase()}.</CardDescription>
      </CardHeader>
      <CardContent className="grid gap-6">
        {datasourceList.slice(0, 3).map((datasource) => (
          <Typography key={datasource.datasource.connectionString} variant="large">
            {datasource.title} on{" "}
            <Button className="m-0 p-0" variant="link">
              {datasource.datasource.url}
            </Button>
          </Typography>
        ))}
      </CardContent>
    </Card>
  );
};

export default DatasourceCard;
