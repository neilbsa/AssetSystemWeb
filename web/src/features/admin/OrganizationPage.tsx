import { useState } from 'react'
import { PageHeader, Segmented } from '@/shared/ui'
import { BranchesTab } from './organization/BranchesTab'
import { CompaniesTab } from './organization/CompaniesTab'
import { DepartmentsTab } from './organization/DepartmentsTab'
import { ItemTypesTab } from './organization/ItemTypesTab'

type Tab = 'branches' | 'departments' | 'itemTypes' | 'companies'

/** Master data that the rest of the app picks from. Branches and departments belong to the current company. */
export function OrganizationPage() {
  const [tab, setTab] = useState<Tab>('branches')
  return (
    <>
      <PageHeader title="Organization" subtitle="Reference data" />
      <div className="mb-4 overflow-x-auto">
        <Segmented<Tab>
          label="Section"
          value={tab}
          onChange={setTab}
          options={[
            { value: 'branches', label: 'Branches' },
            { value: 'departments', label: 'Departments' },
            { value: 'itemTypes', label: 'Item types' },
            { value: 'companies', label: 'Companies' },
          ]}
        />
      </div>
      {tab === 'branches' && <BranchesTab />}
      {tab === 'departments' && <DepartmentsTab />}
      {tab === 'itemTypes' && <ItemTypesTab />}
      {tab === 'companies' && <CompaniesTab />}
    </>
  )
}
